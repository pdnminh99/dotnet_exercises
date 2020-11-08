create table Students
(
    StudentId integer primary key autoincrement,
    FirstName text not null,
    LastName  text,
    Email     text,
    Age       integer default 0
)