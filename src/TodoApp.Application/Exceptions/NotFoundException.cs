namespace TodoApp.Application.Exceptions;

public class NotFoundException(string name, object key) 
    : Exception($"Entity \"{name}\" (key = {key}) was not found.");