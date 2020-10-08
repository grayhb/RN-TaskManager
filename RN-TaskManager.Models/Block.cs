using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RN_TaskManager.Models
{
    [Table("Blocks")]
    public class Block
    {
        [Key]
        public int BlockId { get; set; }
        public string BlockName { get; set; }

        public bool Deleted { get; set; }
    }
}
