using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_GeoTest : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public Guid GId { get; set; } = Guid.NewGuid();
        public string Description { get; set; }
        public Geometry Geom { get; set; }
        public Point Geo { get; set; }


    }
}
