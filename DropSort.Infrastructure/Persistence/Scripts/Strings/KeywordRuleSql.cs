namespace Infrastructure.Persistence.Scripts.Strings;

internal static class KeywordRuleSql
{
    public const string GetEnabled = """
                                         SELECT
                                             id,
                                             keyword,
                                             extensions,
                                             target_folder,
                                             priority,
                                             enabled
                                         FROM keyword_rules
                                         WHERE enabled = 1
                                         ORDER BY priority DESC;
                                     """;

    public const string Insert = """
                                     INSERT INTO keyword_rules (
                                         keyword,
                                         extensions,
                                         target_folder,
                                         priority,
                                         enabled
                                     )
                                     VALUES (
                                         @Keyword,
                                         @Extensions,
                                         @TargetFolder,
                                         @Priority,
                                         @Enabled
                                     );
                                 """;

    public const string Delete = """
                                     DELETE FROM keyword_rules
                                     WHERE id = @id;
                                 """;

    public const string SetEnabled = """
                                         UPDATE keyword_rules
                                         SET enabled = @enabled
                                         WHERE id = @id;
                                     """;
}