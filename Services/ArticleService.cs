using LegendCraft_Backend.Data;
using LegendCraft_Backend.DTOs;
using LegendCraft_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace LegendCraft_Backend.Services
{
    public class ArticleService : IArticleService
    {
        private readonly ApplicationDbContext _context;

        public ArticleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateArticleAsync(ArticleCreateDto dto)
        {
            //Mapeo de datos básicos
            var article = new Article
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                IsPrintOnDemand = dto.IsPrintOnDemand,
                PrintTimeDays = dto.PrintTimeDays
            };

            //Construcción de la lista de Highlights
            for (int i = 0; i < dto.Highlights.Count; i++)
            {
                article.Highlights.Add(new ArticleHighlight
                {
                    Text = dto.Highlights[i],
                    DisplayOrder = i + 1
                });
            }

            // Atributos
            if (dto.AttributeValueIds != null && dto.AttributeValueIds.Any())
            {
                foreach (var attrId in dto.AttributeValueIds)
                {
                    article.ArticleAttributes.Add(new ArticleAttributeValue
                    {
                        AttributeValueId = attrId
                    });
                }
            }

            //Guardado en PostgreSQL
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return article.Id;
        }

        public async Task<List<string>> SaveImagesAsync(int articleId, List<IFormFile> files)
        {
            // Validamos que el artículo exista en la base de datos e incluimos sus imágenes para evitar NullReferenceException
            var article = await _context.Articles
                .Include(a => a.Images)
                .FirstOrDefaultAsync(a => a.Id == articleId);

            if (article == null) throw new Exception("Artículo no encontrado");

            var savedUrls = new List<string>();
            
            // Usamos el ID del artículo para la carpeta (Ej: article_15) 
            // Es más seguro que usar el nombre porque evita caracteres inválidos o cambios de nombre.
            var articleFolderName = $"article_{articleId}";
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", articleFolderName);

            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Obtenemos la extensión. Si viene vacía (ej. porque Angular lo mandó como "blob"), la deducimos del Content-Type
                    var extension = Path.GetExtension(file.FileName);
                    if (string.IsNullOrEmpty(extension))
                    {
                        extension = file.ContentType switch
                        {
                            "image/webp" => ".webp",
                            "image/png" => ".png",
                            "image/jpeg" => ".jpg",
                            _ => ".webp" // Fallback a webp porque nuestro frontend comprime a webp
                        };
                    }

                    // Generamos un nombre único para evitar sobreescribir fotos con el mismo nombre
                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    // Guardamos el archivo físicamente en el disco
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Construimos la URL relativa que se guardará en PostgreSQL
                    var imageUrl = $"/imagenes/{articleFolderName}/{fileName}";

                    // Si es la primera imagen que subimos, la marcamos como principal
                    bool isMainImage = !article.Images.Any();

                    article.Images.Add(new ArticleImage
                    {
                        ImageUrl = imageUrl,
                        IsMain = isMainImage
                    });

                    savedUrls.Add(imageUrl);
                }
            }

            await _context.SaveChangesAsync();
            return savedUrls;
        }

        public async Task SetMainImageAsync(int articleId, int imageId)
        {
            // Obtenemos todas las imágenes asociadas a este artículo
            var images = await _context.ArticleImages
                .Where(i => i.ArticleId == articleId)
                .ToListAsync();

            if (!images.Any())
                throw new Exception("El artículo no tiene imágenes.");

            var selectedImage = images.FirstOrDefault(i => i.Id == imageId);
            if (selectedImage == null)
                throw new Exception("La imagen especificada no existe en este artículo.");

            // Establecemos IsMain = false para todas las imágenes
            foreach (var img in images)
            {
                img.IsMain = false;
            }

            // Establecemos IsMain = true solo para la seleccionada
            selectedImage.IsMain = true;

            // Guardamos los cambios en PostgreSQL
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResultDto<ArticleListResponseDto>> GetAllArticlesAsync(int pageNumber, int pageSize, string? search, List<int>? attributeValues = null)
        {
            // 1. Iniciamos la consulta base, pero NO la ejecutamos todavía
            var query = _context.Articles.AsQueryable();

            // 2. Si el parámetro 'search' tiene texto, agregamos el filtro a la consulta
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(a => 
                    a.Name.ToLower().Contains(searchLower) ||
                    a.Description.ToLower().Contains(searchLower) ||
                    a.Highlights.Any(h => h.Text.ToLower().Contains(searchLower)) ||
                    a.ArticleAttributes.Any(aa => 
                        aa.AttributeValue.Value.ToLower().Contains(searchLower) || 
                        aa.AttributeValue.AttributeType.Name.ToLower().Contains(searchLower))
                );
            }

            // 2.5 Filtrado por atributos (Si enviaron IDs de los checkboxes)
            if (attributeValues != null && attributeValues.Any())
            {
                // Solo traemos artículos que tengan AL MENOS UNO de los atributos seleccionados (Lógica OR)
                query = query.Where(a => a.ArticleAttributes.Any(aa => attributeValues.Contains(aa.AttributeValueId)));
            }

            // 3. Contamos el total de registros que coinciden con el filtro (vital para la paginación)
            var totalRecords = await query.CountAsync();

            // 4. Aplicamos el ordenamiento, la paginación y seleccionamos los datos
            var articles = await query
                .Include(a => a.Images)
                .OrderByDescending(a => a.Id) // Ordenar por Id asegura que el más nuevo esté primero SIEMPRE
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new ArticleListResponseDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Price = a.Price,
                    Stock = a.Stock,
                    IsPrintOnDemand = a.IsPrintOnDemand,
                    PrintTimeDays = a.PrintTimeDays,
                    MainImageUrl = a.Images.FirstOrDefault(i => i.IsMain) != null
                                   ? a.Images.FirstOrDefault(i => i.IsMain)!.ImageUrl
                                   : ""
                })
                .ToListAsync(); // <-- ¡Aquí es donde realmente se ejecuta la consulta en PostgreSQL!

            // 5. Devolvemos el envoltorio con los resultados
            return new PagedResultDto<ArticleListResponseDto>
            {
                Items = articles,
                TotalCount = totalRecords,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ArticleDetailResponseDto?> GetArticleByIdAsync(int id)
        {
            var article = await _context.Articles
                .Include(a => a.Highlights)
                .Include(a => a.Images)
                .Include(a => a.ArticleAttributes)
                    .ThenInclude(aa => aa.AttributeValue) // Traemos el valor (Ej: Monster Hunter)
                    .ThenInclude(av => av.AttributeType)  // Traemos el grupo (Ej: Franquicia)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null) return null;

            return new ArticleDetailResponseDto
            {
                Id = article.Id,
                Name = article.Name,
                Description = article.Description,
                Price = article.Price,
                Stock = article.Stock,
                IsPrintOnDemand = article.IsPrintOnDemand,
                PrintTimeDays = article.PrintTimeDays,
                // Aplanamos las viñetas, ordenadas por el DisplayOrder
                Highlights = article.Highlights.OrderBy(h => h.DisplayOrder).Select(h => h.Text).ToList(),
                // Aplanamos las imágenes
                Images = article.Images.Select(i => new ArticleImageResponseDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    IsMain = i.IsMain
                }).ToList(),
                // Construimos un diccionario clave-valor para los atributos
                Attributes = article.ArticleAttributes
                    .GroupBy(aa => aa.AttributeValue.AttributeType.Name)
                    .ToDictionary(
                        g => g.Key,
                        g => string.Join(", ", g.Select(aa => aa.AttributeValue.Value))
                    )
            };
        }

        public async Task UpdateArticleAsync(int id, ArticleUpdateDto dto)
        {
            var article = await _context.Articles
                .Include(a => a.Highlights) // Incluimos las viñetas para poder reemplazarlas
                .Include(a => a.ArticleAttributes)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null) throw new Exception("Artículo no encontrado");

            article.Name = dto.Name;
            article.Description = dto.Description;
            article.Price = dto.Price;
            article.Stock = dto.Stock;
            article.IsPrintOnDemand = dto.IsPrintOnDemand;
            article.PrintTimeDays = dto.PrintTimeDays;
            article.UpdatedAt = DateTime.UtcNow; // Campo de auditoría

            // Actualizamos las viñetas (la forma más limpia es borrar las viejas y poner las nuevas)
            _context.RemoveRange(article.Highlights);

            for (int i = 0; i < dto.Highlights.Count; i++)
            {
                article.Highlights.Add(new ArticleHighlight
                {
                    Text = dto.Highlights[i],
                    DisplayOrder = i + 1
                });
            }

            _context.RemoveRange(article.ArticleAttributes);
            if (dto.AttributeValueIds != null && dto.AttributeValueIds.Any())
            {
                foreach (var attrId in dto.AttributeValueIds)
                {
                    article.ArticleAttributes.Add(new ArticleAttributeValue
                    {
                        AttributeValueId = attrId
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteArticleAsync(int id)
        {
            var article = await _context.Articles.FindAsync(id);

            if (article == null) throw new Exception("Artículo no encontrado");

            // Eliminación lógica (Soft Delete): No hacemos _context.Remove(article);
            // Cambiamos el estado, y gracias al Global Query Filter que pusimos en el DbContext, 
            // desaparecerá de todas las búsquedas.
            article.IsActive = false;
            article.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteImageAsync(int articleId, int imageId)
        {
            var image = await _context.ArticleImages
                .FirstOrDefaultAsync(i => i.Id == imageId && i.ArticleId == articleId);

            if (image == null) throw new Exception("Imagen no encontrada");

            // Extraemos la ruta relativa desde la URL guardada (Ej: /imagenes/article_1/foto.jpg -> article_1/foto.jpg)
            var relativePath = image.ImageUrl.Replace("/imagenes/", "").Replace("/", Path.DirectorySeparatorChar.ToString());
            
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            var physicalPath = Path.Combine(uploadsPath, relativePath);

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }

            // Borramos el registro de la base de datos
            _context.ArticleImages.Remove(image);

            // Si borramos la imagen principal, podríamos hacer que otra sea principal automáticamente (opcional)

            await _context.SaveChangesAsync();
        }

    }
}
