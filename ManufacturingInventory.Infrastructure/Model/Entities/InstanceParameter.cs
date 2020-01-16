namespace ManufacturingInventory.Infrastructure.Model.Entities {
    public partial class InstanceParameter  {
        public int Id { get; set; }
        public double Value { get; set; }
        public double MinValue { get; set; }
        public double SafeValue { get; set; }

        public bool Tracked { get; set; }
        public byte[] RowVersion { get; set; }

        public int ParameterId { get; set; }
        public Parameter Parameter { get; set; }

        public int PartInstanceId { get; set; }
        public PartInstance PartInstance { get; set; }

        public InstanceParameter() {
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
