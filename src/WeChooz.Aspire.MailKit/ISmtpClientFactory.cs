
using MailKit.Net.Smtp;

namespace WeChooz.Aspire.MailKit;
/// <summary>
/// A factory for creating <see cref="ISmtpClient"/> instances.
/// </summary>
public interface ISmtpClientFactory
{
    /// <summary>
    /// Gets an <see cref="ISmtpClient"/> instance in the connected state
    /// (and that's been authenticated if configured).
    /// </summary>
    /// <param name="cancellationToken">Used to abort client creation and connection.</param>
    /// <returns>A connected (and authenticated) <see cref="ISmtpClient"/> instance.</returns>
    /// <remarks>
    /// Since both the connection and authentication are considered expensive operations,
    /// the <see cref="ISmtpClient"/> returned is intended to be used for the duration of a request
    /// (registered as 'Scoped') and is automatically disposed of.
    /// </remarks>
    Task<ISmtpClient> GetSmtpClientAsync(CancellationToken cancellationToken = default);
}
