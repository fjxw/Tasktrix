using SQLite;

namespace Tasktrix.Models
{
    public class TaskObj
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Name { get; set; }

        public string Notes { get; set; }

        public bool Done { get; set; }

        public bool IsSelected { get; set; }

        public DateTime Date { get; set; }

        public string Priority { get; set; }
        
    }
}