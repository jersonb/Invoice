using System;

namespace Invoice.Client.Models
{
    public abstract class DataObject
    {
        protected DataObject()
        {
            CreateId();
        }

        public string FindId { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public void CreateId()
        {
            FindId = Guid.NewGuid().ToString();
            IsActive = true;
            Created = DateTime.Now;
            IsUpdate(DateTime.Now);
        }

        public void Inactivate(DateTime created)
        {
            IsActive = false;
            IsUpdate(created);
        }

        public void IsUpdate(DateTime created)
        {
            Created = created;
            Updated = DateTime.Now;
        }
    }
}