using GCCars.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GCCars.Application.Migrations
{
    public partial class UpdateCarParametersAddingHealth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = $@"
insert into [Parameters] (CarId, CarParameter, [Value])
	select c.CarId, {(int)CarParameter.Health}, '100'
		from Cars c
		where not exists (select 1 from [Parameters] p where p.CarId = c.CarId and p.CarParameter = {(int)CarParameter.Health})";
            migrationBuilder.Sql(sql, false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sql = $"delete from [Parameters] where CarParameter = {(int)CarParameter.Health}";
            migrationBuilder.Sql(sql, false);
        }
    }
}
