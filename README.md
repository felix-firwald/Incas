# Incas 2

Incas application source code *(Presentation Layer only, including ViewModels)*. 
The application is written in C# programming language (NET Core platform) using WPF framework.

Incas allows you to use so-called classes (storage schemas) to construct models of business entities that you can work with. 
The paradigm of classes in Incas is that you define fields, tables, methods for future objects that will be stored in a database (SQLite, PostgreSQL), templates for rendering documents in .docx and .xlsx formats.

Using classes, you can also customize the appearance of the object editor, write custom logic on Python and hang it on the buttons you create with your icons. With generalizations, you can build classes from separate pre-prepared parts when class elements (field, table part, method) are repeated from class to class.


# Warning
This repository **does not contain** Domain, Persistence and Data Access layers, because all the code of this levels has been moved to the IncasEngine.
