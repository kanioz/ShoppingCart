using System;

namespace ShoppingCart.Core.Model.Base
{
    public abstract class Document : IDocument
    {
        protected Document()
        {
            CreatedAt = DateTime.Now;
        }
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
