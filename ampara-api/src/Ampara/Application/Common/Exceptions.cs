namespace Ampara.Application.Common;

// The resource does not exist or is not visible to the current user (HTTP 404).
public class NotFoundException(string message) : Exception(message);

// Missing or invalid credentials (HTTP 401).
public class UnauthorizedException(string message) : Exception(message);

// The request clashes with existing data, e.g. a duplicate e-mail (HTTP 409).
public class ConflictException(string message) : Exception(message);
