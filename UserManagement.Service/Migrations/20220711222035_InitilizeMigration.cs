using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace User_Management.Migrations
{
    public partial class InitilizeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lg_activity",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "character varying", nullable: true),
                    activity = table.Column<string>(type: "character varying", nullable: true),
                    time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lg_activity", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "md_endpoints",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    key = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    url = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    apikey = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_md_endpoints", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "md_ref_menu",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('lg_ref_menu_id_seq'::regclass)"),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    key = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_md_ref_menu", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "md_role",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('lg_roles_id_seq'::regclass)"),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_admin = table.Column<bool>(type: "boolean", nullable: false),
                    approved_by = table.Column<string>(type: "character varying", nullable: true),
                    approved_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    approval_status = table.Column<bool>(type: "boolean", nullable: true),
                    created_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_md_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ts_task_list",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    activity = table.Column<string>(type: "character varying", nullable: false),
                    time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    type = table.Column<string>(type: "character varying", nullable: false),
                    reference_id = table.Column<int>(type: "integer", nullable: true),
                    created_by = table.Column<string>(type: "character varying", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    approved_by = table.Column<string>(type: "character varying", nullable: true),
                    approved_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ts_task_list", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lg_role",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('lg_role_histories_id_seq'::regclass)"),
                    user = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    activity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying", nullable: true),
                    description = table.Column<string>(type: "character varying", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    is_admin = table.Column<bool>(type: "boolean", nullable: true),
                    created_by = table.Column<string>(type: "character varying", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lg_role", x => x.id);
                    table.ForeignKey(
                        name: "role_id",
                        column: x => x.role_id,
                        principalTable: "md_role",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "md_role_access",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<string>(type: "character varying", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    approved_by = table.Column<string>(type: "character varying", nullable: true),
                    approved_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_md_role_access", x => x.id);
                    table.ForeignKey(
                        name: "ref_md_user_access_md_role",
                        column: x => x.role_id,
                        principalTable: "md_role",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "md_role_user",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<string>(type: "character varying", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    approved_by = table.Column<string>(type: "character varying", nullable: true),
                    approved_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_md_role_user", x => x.id);
                    table.ForeignKey(
                        name: "ref_md_role_user_md_role",
                        column: x => x.role_id,
                        principalTable: "md_role",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "lg_role_access",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_ref_id = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    activity = table.Column<string>(type: "character varying", nullable: false),
                    note = table.Column<string>(type: "character varying", nullable: true),
                    parent_id = table.Column<int>(type: "integer", nullable: true),
                    created_by = table.Column<string>(type: "character varying", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    approved_by = table.Column<string>(type: "character varying", nullable: true),
                    approved_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lg_role_access", x => x.id);
                    table.ForeignKey(
                        name: "ref_lg_user_access_md_role",
                        column: x => x.role_ref_id,
                        principalTable: "md_role",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "ref_parent_id_user_access",
                        column: x => x.parent_id,
                        principalTable: "md_role_access",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "md_role_access_ref",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('md_ref_user_access_id_seq'::regclass)"),
                    ref_menu_id = table.Column<int>(type: "integer", nullable: false),
                    ref_user_access = table.Column<int>(type: "integer", nullable: false),
                    is_view = table.Column<bool>(type: "boolean", nullable: true),
                    is_create = table.Column<bool>(type: "boolean", nullable: true),
                    is_edit = table.Column<bool>(type: "boolean", nullable: true),
                    is_delete = table.Column<bool>(type: "boolean", nullable: true),
                    created_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_md_role_access_ref", x => x.id);
                    table.ForeignKey(
                        name: "ref_user_access",
                        column: x => x.ref_user_access,
                        principalTable: "md_role_access",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "ref_user_access_menu",
                        column: x => x.ref_menu_id,
                        principalTable: "md_ref_menu",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "lg_role_user",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    parent_id = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    note = table.Column<string>(type: "character varying", nullable: true),
                    activity = table.Column<string>(type: "character varying", nullable: false),
                    created_by = table.Column<string>(type: "character varying", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    approved_by = table.Column<string>(type: "character varying", nullable: true),
                    approved_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lg_role_user", x => x.id);
                    table.ForeignKey(
                        name: "ref_lg_role_user_md_role",
                        column: x => x.role_id,
                        principalTable: "md_role",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "ref_to_parent",
                        column: x => x.parent_id,
                        principalTable: "md_role_user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "md_role_user_ref",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    parent_id = table.Column<int>(type: "integer", nullable: false),
                    username = table.Column<string>(type: "character varying", nullable: false),
                    email = table.Column<string>(type: "character varying", nullable: true),
                    full_name = table.Column<string>(type: "character varying", nullable: true),
                    is_approver = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_md_role_user_ref", x => x.id);
                    table.ForeignKey(
                        name: "ref_to_parent",
                        column: x => x.parent_id,
                        principalTable: "md_role_user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "tmp_role_user",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: true),
                    parent_id = table.Column<int>(type: "integer", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    approved_by = table.Column<string>(type: "character varying", nullable: true),
                    approved_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tmp_role_user", x => x.id);
                    table.ForeignKey(
                        name: "ref_tmp_role_user_md_role",
                        column: x => x.role_id,
                        principalTable: "md_role",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "ref_to_parent_role_user",
                        column: x => x.parent_id,
                        principalTable: "md_role_user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "lg_role_access_ref",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('lg_ref_user_access_id_seq'::regclass)"),
                    ref_menu_id = table.Column<int>(type: "integer", nullable: false),
                    ref_user_access = table.Column<int>(type: "integer", nullable: false),
                    is_view = table.Column<bool>(type: "boolean", nullable: true),
                    is_create = table.Column<bool>(type: "boolean", nullable: true),
                    is_edit = table.Column<bool>(type: "boolean", nullable: true),
                    is_delete = table.Column<bool>(type: "boolean", nullable: true),
                    parent_id = table.Column<int>(type: "integer", nullable: true),
                    created_by = table.Column<string>(type: "character varying", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying", nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lg_role_access_ref", x => x.id);
                    table.ForeignKey(
                        name: "ref_to_parent",
                        column: x => x.parent_id,
                        principalTable: "md_role_access_ref",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "ref_user_access",
                        column: x => x.ref_user_access,
                        principalTable: "md_role_access",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "ref_user_access_menu",
                        column: x => x.ref_menu_id,
                        principalTable: "md_ref_menu",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "lg_role_user_ref",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "character varying", nullable: true),
                    full_name = table.Column<string>(type: "character varying", nullable: true),
                    parent_id = table.Column<int>(type: "integer", nullable: false),
                    username = table.Column<string>(type: "character varying", nullable: false),
                    is_approver = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lg_role_user_ref", x => x.id);
                    table.ForeignKey(
                        name: "ref_to_parent",
                        column: x => x.parent_id,
                        principalTable: "lg_role_user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "tmp_role_user_ref",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false, defaultValueSql: "nextval('tmp_role_user_ref_Id_seq'::regclass)"),
                    email = table.Column<string>(type: "character varying", nullable: true),
                    full_name = table.Column<string>(type: "character varying", nullable: true),
                    parent_id = table.Column<string>(type: "character varying", nullable: false),
                    username = table.Column<string>(type: "character varying", nullable: false),
                    is_approver = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tmp_role_user_ref", x => x.id);
                    table.ForeignKey(
                        name: "ref_to_parent",
                        column: x => x.parent_id,
                        principalTable: "tmp_role_user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_lg_role_role_id",
                table: "lg_role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_lg_role_access_parent_id",
                table: "lg_role_access",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_lg_role_access_role_ref_id",
                table: "lg_role_access",
                column: "role_ref_id");

            migrationBuilder.CreateIndex(
                name: "IX_lg_role_access_ref_parent_id",
                table: "lg_role_access_ref",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_lg_role_access_ref_ref_menu_id",
                table: "lg_role_access_ref",
                column: "ref_menu_id");

            migrationBuilder.CreateIndex(
                name: "IX_lg_role_access_ref_ref_user_access",
                table: "lg_role_access_ref",
                column: "ref_user_access");

            migrationBuilder.CreateIndex(
                name: "IX_lg_role_user_parent_id",
                table: "lg_role_user",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_lg_role_user_role_id",
                table: "lg_role_user",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_lg_role_user_ref_parent_id",
                table: "lg_role_user_ref",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_md_role_access_role_id",
                table: "md_role_access",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_md_role_access_ref_ref_menu_id",
                table: "md_role_access_ref",
                column: "ref_menu_id");

            migrationBuilder.CreateIndex(
                name: "IX_md_role_access_ref_ref_user_access",
                table: "md_role_access_ref",
                column: "ref_user_access");

            migrationBuilder.CreateIndex(
                name: "IX_md_role_user_role_id",
                table: "md_role_user",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_md_role_user_ref_parent_id",
                table: "md_role_user_ref",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_tmp_role_user_parent_id",
                table: "tmp_role_user",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_tmp_role_user_role_id",
                table: "tmp_role_user",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_tmp_role_user_ref_parent_id",
                table: "tmp_role_user_ref",
                column: "parent_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lg_activity");

            migrationBuilder.DropTable(
                name: "lg_role");

            migrationBuilder.DropTable(
                name: "lg_role_access");

            migrationBuilder.DropTable(
                name: "lg_role_access_ref");

            migrationBuilder.DropTable(
                name: "lg_role_user_ref");

            migrationBuilder.DropTable(
                name: "md_endpoints");

            migrationBuilder.DropTable(
                name: "md_role_user_ref");

            migrationBuilder.DropTable(
                name: "tmp_role_user_ref");

            migrationBuilder.DropTable(
                name: "ts_task_list");

            migrationBuilder.DropTable(
                name: "md_role_access_ref");

            migrationBuilder.DropTable(
                name: "lg_role_user");

            migrationBuilder.DropTable(
                name: "tmp_role_user");

            migrationBuilder.DropTable(
                name: "md_role_access");

            migrationBuilder.DropTable(
                name: "md_ref_menu");

            migrationBuilder.DropTable(
                name: "md_role_user");

            migrationBuilder.DropTable(
                name: "md_role");
        }
    }
}
