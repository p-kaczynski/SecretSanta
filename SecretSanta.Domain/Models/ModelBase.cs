using System.Text;

namespace SecretSanta.Domain.Models
{
    public abstract class ModelBase
    {
        public long Id { get; set; }
        public byte[] IV()=> Encoding.UTF8.GetBytes(IVSource);

        protected abstract string IVSource { get; }
    }
}