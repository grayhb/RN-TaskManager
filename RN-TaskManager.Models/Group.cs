using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RN_TaskManager.Models
{
    [Table("Groups")]
    public class Group
    {
        [Key]
        public int GroupId { get; set; }

        public string GroupNumber { get; set; }
        public string GroupName { get; set; }

        public bool Deleted { get; set; }

    }
}
