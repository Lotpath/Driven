using System;

namespace Driven.SampleDomain
{
    public class TenantId
    {
        private string _id;

        public TenantId(string id) : this()
        {
            SetId(id);
        }

        public TenantId(TenantId id) : this()
        {
            SetId(id._id);
        }

        private TenantId()
        {
        }

        public override string ToString()
        {
            return "TenantId [_id=" + _id + "]";
        }

        private void SetId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("The id must be provided.");
            }

            Guid parsed;
            if (!Guid.TryParse(id, out parsed))
            {
                throw new ArgumentException("The id must be a valid Guid.");
            }

            _id = id;
        }

        public string Id()
        {
            return _id;
        }
    }
}