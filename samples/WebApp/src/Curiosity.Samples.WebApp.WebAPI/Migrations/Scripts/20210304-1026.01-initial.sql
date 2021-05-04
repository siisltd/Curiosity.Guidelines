-- миграция с основними таблицами
-- файлы
CREATE TABLE files
(
    id                  bigserial                                      NOT NULL,
    created             timestamp DEFAULT timezone('UTC'::text, NOW()) NOT NULL, -- по умолчанию задаёт время создания создания сущности
    user_id             bigint                                         NOT NULL,
    user_file_name      varchar(200)                                   NOT NULL,
    storage_file_path   varchar(200)                                   NOT NULL,
    storage_file_name   varchar(200)                                   NOT NULL,
    
    CONSTRAINT pk_files PRIMARY KEY (id),
    CONSTRAINT fk_files_to_users FOREIGN KEY (user_id) REFERENCES "AspNetUsers"("Id") ON DELETE CASCADE -- ссылка на другую таблицу
);

CREATE UNIQUE INDEX uix_files_storagefilename ON files USING btree (lower((storage_file_name)::text)); -- уникальный индекс (гарантия консистентности данных)
CREATE INDEX fki_files_userid ON files USING btree (user_id);                                          -- индекс для оптимизации FOREIGN KEY

ALTER TABLE files OWNER TO %USER%;
