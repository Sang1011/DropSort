CREATE TABLE IF NOT EXISTS settings (
                                        key TEXT PRIMARY KEY,
                                        value TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS logs (
                                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    level TEXT NOT NULL,
                                    message TEXT NOT NULL,
                                    file_name TEXT,
                                    source_path TEXT,
                                    target_path TEXT,
                                    created_at TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS watch_paths (
                                           id INTEGER PRIMARY KEY AUTOINCREMENT,
                                           path TEXT NOT NULL UNIQUE,
                                           enabled INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS keyword_rules (
                                             id INTEGER PRIMARY KEY AUTOINCREMENT,
                                             keyword TEXT NOT NULL,
                                             extensions TEXT,
                                             target_folder TEXT NOT NULL,
                                             priority INTEGER NOT NULL DEFAULT 0,
                                             enabled INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS file_tasks (
                                          id TEXT PRIMARY KEY,                  -- Guid
                                          full_path TEXT NOT NULL,
                                          file_name TEXT NOT NULL,
                                          extension TEXT NOT NULL,
                                          size_in_bytes INTEGER NOT NULL,
                                          source_drive TEXT NOT NULL,

                                          category INTEGER NOT NULL,             -- enum FileCategory
                                          target_path TEXT NOT NULL,

                                          status INTEGER NOT NULL,               -- enum FileTaskStatus

                                          created_at TEXT NOT NULL,
                                          completed_at TEXT
);