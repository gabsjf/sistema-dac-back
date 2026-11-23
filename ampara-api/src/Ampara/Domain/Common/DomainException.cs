namespace Ampara.Domain.Common;

// A business rule was violated. The message is shown to the user, so it is written in pt-BR.
public class DomainException(string message) : Exception(message);
