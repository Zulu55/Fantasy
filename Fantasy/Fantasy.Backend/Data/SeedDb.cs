using Fantasy.Backend.Helpers;
using Fantasy.Backend.UnitsOfWork.Interfaces;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Data;

public class SeedDb
{
    private readonly DataContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly IUsersUnitOfWork _usersUnitOfWork;

    public SeedDb(DataContext context, IFileStorage fileStorage, IUsersUnitOfWork usersUnitOfWork)
    {
        _context = context;
        _fileStorage = fileStorage;
        _usersUnitOfWork = usersUnitOfWork;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await CheckCountriesAsync();
        await CheckTeamsAsync();
        await CheckRolesAsync();
        await CheckUserAsync("Juan", "Zuluaga", "zulu@yopmail.com", "322 311 4620", UserType.Admin);
        await CheckTournamentsAsync();
    }

    private async Task CheckTournamentsAsync()
    {
        if (!_context.TournamentTeams.Any())
        {
            var colombia = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Colombia")!;
            var peru = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Peru");
            var ecuador = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Ecuador");
            var venezuela = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Venezuela");
            var brazil = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Brazil");
            var argentina = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Argentina");
            var uruguay = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Uruguay");
            var chile = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Chile");
            var bolivia = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Bolivia");
            var paraguay = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Paraguay");

            var unitedStates = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "United States");
            var canada = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Canada");
            var mexico = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Mexico");
            var panama = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Panama");
            var costaRica = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Costa Rica ");
            var honduras = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Honduras");
            var jamaica = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Jamaica");
            var guatemala = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Guatemala");
            var barbados = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Barbados");
            var dominica = await _context.Teams.FirstOrDefaultAsync(x => x.Name == "Dominica");

            var copaAmerica = new Tournament
            {
                IsActive = true,
                Name = "Copa America - 2025",
                TournamentTeams =
                [
                    new TournamentTeam { Team = colombia! },
                    new TournamentTeam { Team = peru! },
                    new TournamentTeam { Team = ecuador! },
                    new TournamentTeam { Team = venezuela! },
                    new TournamentTeam { Team = brazil! },
                    new TournamentTeam { Team = argentina! },
                    new TournamentTeam { Team = uruguay! },
                    new TournamentTeam { Team = chile! },
                    new TournamentTeam { Team = bolivia! },
                    new TournamentTeam { Team = paraguay! },
                    new TournamentTeam { Team = unitedStates! },
                    new TournamentTeam { Team = canada! },
                ]
            };

            var copaOro = new Tournament
            {
                IsActive = true,
                Name = "Copa Oro - 2025",
                TournamentTeams =
                [
                    new TournamentTeam { Team = unitedStates! },
                    new TournamentTeam { Team = canada! },
                    new TournamentTeam { Team = mexico! },
                    new TournamentTeam { Team = panama! },
                    new TournamentTeam { Team = costaRica! },
                    new TournamentTeam { Team = honduras! },
                    new TournamentTeam { Team = jamaica! },
                    new TournamentTeam { Team = guatemala! },
                    new TournamentTeam { Team = barbados! },
                    new TournamentTeam { Team = dominica! },
                    new TournamentTeam { Team = colombia! },
                    new TournamentTeam { Team = uruguay! },
                ]
            };

            _context.Tournaments.Add(copaAmerica);
            _context.Tournaments.Add(copaOro);
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckRolesAsync()
    {
        await _usersUnitOfWork.CheckRoleAsync(UserType.Admin.ToString());
        await _usersUnitOfWork.CheckRoleAsync(UserType.User.ToString());
    }

    private async Task<User> CheckUserAsync(string firstName, string lastName, string email, string phone, UserType userType)
    {
        var user = await _usersUnitOfWork.GetUserAsync(email);
        if (user == null)
        {
            var country = await _context.Countries.FirstOrDefaultAsync(x => x.Name == "Colombia");
            user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email,
                PhoneNumber = phone,
                Country = country!,
                UserType = userType,
            };

            await _usersUnitOfWork.AddUserAsync(user, "123456");
            await _usersUnitOfWork.AddUserToRoleAsync(user, userType.ToString());

            var token = await _usersUnitOfWork.GenerateEmailConfirmationTokenAsync(user);
            await _usersUnitOfWork.ConfirmEmailAsync(user, token);
        }

        return user;
    }

    private async Task CheckCountriesAsync()
    {
        if (!_context.Countries.Any())
        {
            var countriesSQLScript = File.ReadAllText("Data\\Countries.sql");
            await _context.Database.ExecuteSqlRawAsync(countriesSQLScript);
        }
    }

    private async Task CheckTeamsAsync()
    {
        if (!_context.Teams.Any())
        {
            foreach (var country in _context.Countries)
            {
                var imagePath = string.Empty;
                var filePath = $"{Environment.CurrentDirectory}\\Images\\Flags\\{country.Name}.png";
                if (File.Exists(filePath))
                {
                    var fileBytes = File.ReadAllBytes(filePath);
                    imagePath = await _fileStorage.SaveFileAsync(fileBytes, "jpg", "teams");
                }
                _context.Teams.Add(new Team { Name = country.Name, Country = country!, Image = imagePath });
            }

            await _context.SaveChangesAsync();
        }
    }
}