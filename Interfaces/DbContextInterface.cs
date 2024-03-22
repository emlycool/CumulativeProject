using MySql.Data.MySqlClient;

namespace CumulativeProject.Interfaces
{
    public interface DbContextInterface
    {
        MySqlConnection AccessDatabase();
    }
}
