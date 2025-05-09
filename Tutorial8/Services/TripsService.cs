using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _conn;
    public TripsService(IConfiguration config)
    {
        _conn = config.GetConnectionString("DefaultConnection");
    }

    public async Task<List<TripDTO>> GetTrips()
    {
        var list = new List<TripDTO>();
        await using var c = new SqlConnection(_conn);
        await c.OpenAsync();
        await using var cmd = new SqlCommand("SELECT IdTrip, Name FROM Trip", c);
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            list.Add(new TripDTO
            {
                Id = r.GetInt32(r.GetOrdinal("IdTrip")),
                Name = r.GetString(r.GetOrdinal("Name")),
                Countries = new List<CountryDTO>()
            });
        }
        return list;
    }

    public async Task<TripDTO> GetTrip(int id)
    {
        TripDTO trip = null;
        await using var c = new SqlConnection(_conn);
        await c.OpenAsync();
        const string sql = @"
            SELECT t.IdTrip, t.Name, c.Name AS CountryName
            FROM Trip t
            LEFT JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip
            LEFT JOIN Country c ON ct.IdCountry = c.IdCountry
            WHERE t.IdTrip = @id";
        await using var cmd = new SqlCommand(sql, c);
        cmd.Parameters.AddWithValue("@id", id);
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
        {
            if (trip == null)
            {
                trip = new TripDTO
                {
                    Id = r.GetInt32(r.GetOrdinal("IdTrip")),
                    Name = r.GetString(r.GetOrdinal("Name")),
                    Countries = new List<CountryDTO>()
                };
            }
            if (!r.IsDBNull(r.GetOrdinal("CountryName")))
            {
                trip.Countries.Add(new CountryDTO
                {
                    Name = r.GetString(r.GetOrdinal("CountryName"))
                });
            }
        }
        return trip;
    }
}
