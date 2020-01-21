using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;

namespace ManufacturingInventory.Infrastructure.Model.Entities {
    public partial class Unit  {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int Power { get; set; }
        public int Exponent { get; set; }
        public byte[] RowVersion { get; set; }

        public ICollection<Parameter> Parameters { get; set; }

        public Unit() {
            this.Parameters = new HashSet<Parameter>();
        }

        public Unit(string name, string shortName, int power, int exponent) : this() {
            this.Name = name;
            this.ShortName = shortName;
            this.Power = power;
            this.Exponent = exponent;
        }
    }
}
