using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DTOs {
    public class DTOIGDBGameExpanded {
        public int Id { get; set; }

        public string Name { get; set; }

        public double? Rating { get; set; }

        [JsonProperty("cover")]
        public DTOIGDBCoverExpanded CoverData { get; set; }

        public int? Category { get; set; }

        // Propiedades adicionales
        public long? FirstReleaseDate { get; set; }

        public string? Summary { get; set; }

        /// <summary>
        /// Obtiene el ID de la portada para construir la URL
        /// </summary>
        public string GetCoverUrl() {
            if (CoverData?.ImageId == null) {
                return null;
            }
            return $"https://images.igdb.com/igdb/image/upload/t_cover_small/{CoverData.ImageId}.jpg";
        }
    }

    public class DTOIGDBCoverExpanded {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("image_id")]
        public string ImageId { get; set; }
    }
}
