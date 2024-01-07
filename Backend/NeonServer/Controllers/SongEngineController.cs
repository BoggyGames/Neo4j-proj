using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System;

namespace NeonServer.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class SongEngineController : ControllerBase
    {
        private readonly ILogger<SongEngineController> _logger;
        private readonly IDriver _driver;
        private const string uri = "bolt://localhost:7687";
        private const string user = "neo4j";
        private const string password = "neo4j";

        public SongEngineController(ILogger<SongEngineController> logger)
        {
            _logger = logger;
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        [HttpGet(Name = "GetSongRecs")]
        public async Task<IEnumerable<Song>> Get(string startSong)
        {
            using var session = _driver.AsyncSession();
            var transaction = session.ExecuteReadAsync(
                async tx =>
                {
                    var cursor = await tx.RunAsync(

                        "MATCH(selectedSong: Song { title: $startSong}) < -[:PRODUCED]-(artist: Artist) " +
                        "RETURN selectedSong AS `Songs` " +
                        "UNION MATCH(selectedSong:Song { title: $startSong})< -[:PRODUCED] - (artist: Artist) " +
                        "MATCH(artist) - [:PRODUCED]->(otherSong: Song) " +
                        "RETURN otherSong AS `Songs` " +
                        "UNION MATCH(selectedSong:Song { title: $startSong})< -[:PRODUCED] - (artist: Artist) " +
                        "MATCH(collaboratorArtist) - [:PRODUCED]->(collaboratorSong: Song) < -[:PRODUCED] - (artist) " +
                        "WHERE collaboratorArtist<> artist " +
                        "MATCH(otherGuysTrack: Song) < -[:PRODUCED] - (collaboratorArtist) " +
                        "RETURN otherGuysTrack AS `Songs` " +
                        "UNION MATCH(song:Song) " +
                        "RETURN song AS `Songs`"
                        ,
                        new { startSong });

                        var records = await cursor.ToListAsync();
                        var songs = records
                            .Select(x => x["Songs"])
                            .ToArray();

                    return songs;
                    //return result.Single()[0].As<string>();

                });

            var res = await transaction;

            List<Song> songs = new List<Song>();

            for (int i = 0; i < res.Length; i++)
            {
                INode node = res[i] as INode;
                Song song = new Song();
                song.Id = i;
                song.Title = node.Properties["title"].ToString();
                song.Album = node.Properties["album"].ToString();
                song.Genre = node.Properties["genre"].ToString();
                song.Mood = node.Properties["mood"].ToString();
                songs.Add(song);
            }

            return songs;
        }
    }
}
