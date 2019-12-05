using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingInventory.Common.Model.Entities {
    public partial class Parameter  {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int UnitId { get; set; }
        public virtual Unit Unit { get; set; }

        public byte[] RowVersion { get; set; }

        public virtual ICollection<InstanceParameter> InstanceParameters { get; set; }

        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }

        public Parameter() {
            this.InstanceParameters = new HashSet<InstanceParameter>();
        }

        public Parameter(string name, string description) : this() {
            this.Name = name;
            this.Description = description;
        }
    }

    public partial class Unit  {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int Power { get; set; }
        public int Exponent { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<Parameter> Parameters { get; set; }

        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }

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



    public partial class InstanceParameter  {
        public int Id { get; set; }
        public double Value { get; set; }
        public double MinValue { get; set; }
        public double SafeValue { get; set; }
        public bool Tracked { get; set; }
        public byte[] RowVersion { get; set; }

        public int ParameterId { get; set; }
        public virtual Parameter Parameter { get; set; }

        public int PartInstanceId { get; set; }
        public virtual PartInstance PartInstance { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

        public DateTime? Inserted { get; set; }
        public DateTime? Updated { get; set; }


        public InstanceParameter() {
            this.Transactions = new ObservableHashSet<Transaction>();
        }

        public InstanceParameter(PartInstance part, Parameter parameter) : this() {
            this.PartInstance = part;
            this.Parameter = parameter;
        }

        public void SetValues(double value, double min, double safe) {
            this.Value = value;
            this.MinValue = min;
            this.SafeValue = safe;
        }
    }
}
