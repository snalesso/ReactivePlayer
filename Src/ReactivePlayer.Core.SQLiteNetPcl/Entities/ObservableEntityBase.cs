using System;

namespace ReactivePlayer.Core.Entities
{
    public abstract class ObservableEntityBase  : ObservableObjectBase, IEntityBase
    {
        protected ObservableEntityBase() { }

        private int _id;
        public int Id
        {
            get { return this._id; }
            set
            {
                // if the ID is already set
                if (this._id > 0)
                    throw new InvalidOperationException($"A {this.GetType().DeclaringType.Name}'s {nameof(EntityBase.Id)} cannot be set more than once.");
                // if trying to set the ID to 0 or less
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(EntityBase.Id), value, $"A {this.GetType().DeclaringType.Name}'s {nameof(EntityBase.Id)} must be greater than 0.");

                this._id = value;
            }
        }
    }
}