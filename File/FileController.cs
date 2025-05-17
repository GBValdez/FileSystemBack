using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using back.File.Dtos;
using back.models;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;
using project.users;
using project.utils;
using project.utils.dto;

namespace back.File
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMINISTRATOR")]
    public class FileController : controllerCommons<Files, Files, FileDto, FileQueryDto, object, long>
    {
        private string rutaBase = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        private UserManager<userEntity> userManager;
        public FileController(DBProyContext context, IMapper mapper, IConfiguration configuration, UserManager<userEntity> userManagerTemp) : base(context, mapper)
        {
            this.rutaBase = configuration["RutaBase"];
            this.userManager = userManagerTemp;
            // this.rutaBase = this.rutaBase.Replace("/", Path.DirectorySeparatorChar.ToString());

        }
        public override async Task<ActionResult<FileDto>> post(Files newRegister, [FromQuery] object queryParams)
        {
            return BadRequest("No implementado");
        }
        public override async Task<ActionResult> put(Files entityCurrent, [FromRoute] long id, [FromQuery] object queryCreation)
        {
            return BadRequest("No implementado");
        }

        public override async Task<ActionResult> delete(long id)
        {
            return BadRequest("No implementado");
        }

        protected override async Task<IQueryable<Files>> modifyGet(IQueryable<Files> query, FileQueryDto queryParams)
        {
            query = query.Include(x => x.userUpdate);
            if (queryParams.dateFrom != null && queryParams.dateTo != null)
            {
                query = query.Where(x => x.createAt >= queryParams.dateFrom && x.createAt <= queryParams.dateTo);
            }
            if (queryParams.userName != null)
            {
                userEntity user = await userManager.FindByNameAsync(queryParams.userName);
                if (user == null)
                {
                    return query;
                }
                query = query.Where(x => x.userUpdateId == user.Id);
            }

            return query;
        }
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> uploadFile([FromForm] IFormFile file)
        {

            try
            {
                const long maxSize = 5 * 1024 * 1024; // 10 MB (puedes cambiar esto según el tamaño máximo permitido)

                if (file == null || file.Length == 0)
                    return BadRequest(new errorMessageDto("El archivo no puede ser nulo o vacío"));
                if (file.Length > maxSize)
                    return BadRequest(new errorMessageDto("El archivo es demasiado grande. El tamaño máximo permitido es de 5 MB."));

                using var stream = file!.OpenReadStream();
                PdfReader reader = new PdfReader(stream);
                PdfDocument? pdfDoc = null;
                try
                {
                    pdfDoc = new PdfDocument(reader);
                }
                catch (PdfException)
                {
                    return BadRequest(new errorMessageDto("El Pdf está encriptado, carga un documento que sea valido para cargar."));
                }
                int totalPages = pdfDoc.GetNumberOfPages();
                if (totalPages == 0)
                {
                    return BadRequest(new errorMessageDto("El Pdf no tiene páginas"));
                }
                for (int i = 1; i <= totalPages; i++)
                {
                    var page = pdfDoc.GetPage(i);
                    var content = PdfTextExtractor.GetTextFromPage(page);
                    if (!string.IsNullOrWhiteSpace(content)) continue;
                    return BadRequest(new errorMessageDto("El Pdf no tiene contenido"));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new errorMessageDto("Error al subir el archivo: " + ex.Message));
            }
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // 1. Crear y guardar registro inicial
                Files fileModel = new Files { Name = "NULL", OriginalName = "NULL" };
                await context.Files.AddAsync(fileModel);
                await context.SaveChangesAsync();

                // 2. Crear nombre y ruta
                var nuevoNombre = $"Archivos_{fileModel.Id}.pdf";
                var path = Path.Combine(rutaBase, nuevoNombre);

                // 3. Guardar archivo
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 4. Actualizar nombre y confirmar
                fileModel.Name = nuevoNombre;
                fileModel.OriginalName = file.FileName;
                await context.SaveChangesAsync();

                await transaction.CommitAsync();
                return Ok(new { mensaje = "Archivo subido", nombre = file.FileName });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new errorMessageDto("Error al subir el archivo: " + ex.Message));
            }
        }

        [HttpGet("download/{id}")]
        public async Task<ActionResult> downloadFile(long id)
        {
            Files file = await context.Files.FirstOrDefaultAsync(x => x.Id == id);
            if (file == null)
                return NotFound(new errorMessageDto("El archivo no existe"));

            string path = Path.Combine(rutaBase, file.Name);
            if (!System.IO.File.Exists(path))
                return NotFound(new errorMessageDto("El archivo no se encuentra físicamente"));

            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return File(stream, "application/octet-stream", file.OriginalName);
        }

    }

}