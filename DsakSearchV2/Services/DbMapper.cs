using DsakSearchV2.DTOs;
using DsakSearchV2.Services.Db;

namespace DsakSearchV2.Services
{
    public static class DbMapper
    {
        public static List<DsakDto> MapToDsakDto(IEnumerable<DummyRecord> dummyRecord)
        {
            List<DsakDto> dtos = new();

            foreach (var record in dummyRecord)
            {
                dtos.Add(new DsakDto
                {
                    CategoryId = record.CategoryId,
                    Name = record.Name,
                    Rank = record.Rank,
                    Tooltip = record.Tooltip,
                    Registered = record.Registered,
                    Updated = record.Updated,
                    Deleted = IsTrue(record.Deleted)
                });
            }

            return dtos;
        }

        public static bool IsTrue(int value) => value == 0;
    }
}
