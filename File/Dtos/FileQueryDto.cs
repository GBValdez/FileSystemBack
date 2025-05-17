using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back.File.Dtos
{
    public class FileQueryDto
    {
        public DateTime? dateTo { get; set; }
        public DateTime? dateFrom { get; set; }
        public string? userName { get; set; }
    }
}