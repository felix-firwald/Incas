# Incas 2

**Incas & IncasEngine Author (developer):** D. Magon

![Core](https://github.com/user-attachments/assets/88208884-ffce-4644-8f5e-f36391788ba5)

Incas application source code *(Presentation Layer only, including ViewModels)*. 
The application is written in C# programming language (NET Core platform) using WPF framework.

![Безымянный-1](https://github.com/user-attachments/assets/bea6609f-3865-4d84-b4e9-ae416d4341bc)


## Admin Panel

Looks like this
![image](https://github.com/user-attachments/assets/b9376bfd-673c-410a-b72e-f48f8a1ae675)


## Components

The components define the main areas of work within your workspace. Each component has a name, description, and icon (the icon can be taken from [here](https://fonts.google.com/icons?icon.style=Rounded)).

![image](https://github.com/user-attachments/assets/fc7d5a1d-ef9a-4a84-a195-3bc74f4ec024)


## Classes

Incas allows you to use so-called classes (storage schemas) to construct models of business entities that you can work with. 

![image](https://github.com/user-attachments/assets/a8be9fe8-f261-49bc-8faf-aec78c346335)

The paradigm of classes in Incas is that you define fields, tables, methods for future objects that will be stored in a database (SQLite, PostgreSQL), templates for rendering documents in .docx and .xlsx formats.

Using classes, you can also customize the appearance of the object editor, write custom logic on Python and hang it on the buttons you create with your icons. With generalizations, you can build classes from separate pre-prepared parts when class elements (field, table part, method) are repeated from class to class.

![image](https://github.com/user-attachments/assets/a6b46092-87ec-46c3-868d-4cc33f91b2b1)

![image](https://github.com/user-attachments/assets/9ae0d1e1-cf95-45be-91a7-61dfd1a96a8b)

You can also manage the states of your objects (declaratively describe Incas at what point in time which fields should be available for modification, visible or mandatory for the user)

![image](https://github.com/user-attachments/assets/3edaad63-6795-4c90-ae94-7ad04aea432e)


## Permissions

Using authorization groups (service class, class elements can also be overridden in a separate window), you can restrict access of users of a certain group to a certain class or vice versa, grant restricted access. The number of authorization groups is not limited. Customizing access to classes looks like this:
![image](https://github.com/user-attachments/assets/82a2a0e1-074d-40f1-9182-74e04119e142)