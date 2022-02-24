using System;
using System.Diagnostics;

namespace PrototypeEventWireup
{
    /// <summary>
    /// Provides a static set of methods for writing events to the application event log.  Information, Errors and
    /// Warnings can be logged.
    /// </summary>
    public sealed class WindowsEventLogger
    {
            private WindowsEventLogger()
            {
                //
                // Do not use this constructor - all methods of this class are static.
                // This private constructor is here to prevent the CLR from constructing
                // a default public one, which could degrade performance.
                //
            }

            #region Static Methods for writing Information to Application event log
            /// <summary>
            /// Writes an Information entry into the Windows Application event log.
            /// </summary>
            /// <param name="eventMessage">Descriptive text detailing the nature of the event.</param>
            /// <param name="eventID">Integer value specifying the event ID.</param>
            public static void LogInformation(string eventMessage, int eventID)
            {
                LogInformation(eventMessage, eventID, Properties.Resources.EventLogTaryn, Properties.Resources.ApplicationEventLog);
            }

            /// <summary>
            /// Writes an Information entry into the Windows Application event log.
            /// </summary>
            /// <param name="eventMessage">Descriptive text detailing the nature of the event.</param>
            /// <param name="eventID">Integer value specifying the event ID.</param>
            /// <param name="eventSource">The event source; this is typically the application's name.</param>
            public static void LogInformation(string eventMessage, int eventID, string eventSource)
            {
                LogInformation(eventMessage, eventID, eventSource, Properties.Resources.ApplicationEventLog);
            }

            /// <summary>
            /// Writes an Information entry into the Windows Application event log.
            /// </summary>
            /// <param name="eventMessage">Descriptive text detailing the nature of the event.</param>
            /// <param name="eventID">Integer value specifying the event ID.</param>
            /// <param name="eventSource">The event source; this is typically the application's name.</param>
            /// <param name="eventLog">The Windows event log to write into.  Standard logs are Application, Security and System.</param>
            public static void LogInformation(string eventMessage, int eventID, string eventSource, string eventLog)
            {
                try
                {
                    EventLog.WriteEntry(eventSource, eventMessage, EventLogEntryType.Information, eventID);
                }
                catch (Exception)
                {

                }
            }
            #endregion

            #region Static Methods for writing Error to Application event log
            /// <summary>
            /// Writes an Error entry into the Windows Application event log.
            /// </summary>
            /// <param name="eventMessage">Descriptive text detailing the nature of the error.</param>
            /// <param name="eventID">Integer value specifying the event ID.</param>
            public static void LogError(string eventMessage, int eventID)
            {
                LogError(eventMessage, eventID, Properties.Resources.EventLogTaryn, Properties.Resources.ApplicationEventLog);
            }

            /// <summary>
            /// Writes an Error entry into the Windows Application event log.
            /// </summary>
            /// <param name="eventMessage">Descriptive text detailing the nature of the error.</param>
            /// <param name="eventID">Integer value specifying the event ID.</param>
            /// <param name="eventSource">The event source; this is typically the application's name.</param>
            public static void LogError(string eventMessage, int eventID, string eventSource)
            {
                LogError(eventMessage, eventID, eventSource, Properties.Resources.ApplicationEventLog);
            }

            /// <summary>
            /// Writes an Error entry into the Windows Application event log.
            /// </summary>
            /// <param name="eventMessage">Descriptive text detailing the nature of the error.</param>
            /// <param name="eventID">Integer value specifying the event ID.</param>
            /// <param name="eventSource">The event source; this is typically the application's name.</param>
            /// <param name="eventLog">The Windows event log to write into.  Standard logs are Application, Security and System.</param>
            public static void LogError(string eventMessage, int eventID, string eventSource, string eventLog)
            {
                try
                {
                    EventLog.WriteEntry(eventSource, eventMessage, EventLogEntryType.Error, eventID);
                }
                catch (Exception)
                {

                }
            }
            #endregion

            #region Static Methods for writing Warning to Application event log
            /// <summary>
            /// Writes a Warning entry into the Windows Application event log.
            /// </summary>
            /// <param name="eventMessage">Descriptive text detailing the nature of the warning.</param>
            /// <param name="eventID">Integer value specifying the event ID.</param>
            public static void LogWarning(string eventMessage, int eventID)
            {
                LogWarning(eventMessage, eventID, Properties.Resources.EventLogTaryn, Properties.Resources.ApplicationEventLog);
            }

            /// <summary>
            /// Writes a Warning entry into the Windows Application event log.
            /// </summary>
            /// <param name="eventMessage">Descriptive text detailing the nature of the warning.</param>
            /// <param name="eventID">Integer value specifying the event ID.</param>
            /// <param name="eventSource">The event source; this is typically the application's name.</param>
            public static void LogWarning(string eventMessage, int eventID, string eventSource)
            {
                LogWarning(eventMessage, eventID, eventSource, Properties.Resources.ApplicationEventLog);
            }

            /// <summary>
            /// Writes a Warning entry into the Windows Application event log.
            /// </summary>
            /// <param name="eventMessage">Descriptive text detailing the nature of the warning.</param>
            /// <param name="eventID">Integer value specifying the event ID.</param>
            /// <param name="eventSource">The event source; this is typically the application's name.</param>
            /// <param name="eventLog">The Windows event log to write into.  Standard logs are Application, Security and System.</param>
            public static void LogWarning(string eventMessage, int eventID, string eventSource, string eventLog)
            {
                try
                {
                    EventLog.WriteEntry(eventSource, eventMessage, EventLogEntryType.Warning, eventID);
                }
                catch (Exception)
                {

                }
            }
            #endregion

            #region Static Methods for writing Success Audit to Security event log
            /// <summary>
            /// Writes a Success Audit entry into the Windows Security event log.
            /// </summary>
            /// <param name="eventMessage">Descriptive text detailing the nature of the audit.</param>
            /// <param name="eventID">Integer value specifying the event ID.</param>
            public static void LogSuccessAudit(string eventMessage, int eventID)
            {
                LogSuccessAudit(eventMessage, eventID, Properties.Resources.EventLogTaryn, "Security");
            }

            /// <summary>
            /// Writes a Success Audit entry into the Windows Security event log.
            /// </summary>
            /// <param name="eventMessage">Descriptive text detailing the nature of the audit.</param>
            /// <param name="eventID">Integer value specifying the event ID.</param>
            /// <param name="eventSource">The event source; this is typically the application's name.</param>
            public static void LogSuccessAudit(string eventMessage, int eventID, string eventSource)
            {
                LogSuccessAudit(eventMessage, eventID, eventSource, "Security");
            }

            /// <summary>
            /// Writes a Success Audit entry into the Windows Security event log.
            /// </summary>
            /// <param name="eventMessage">Descriptive text detailing the nature of the audit.</param>
            /// <param name="eventID">Integer value specifying the event ID.</param>
            /// <param name="eventSource">The event source; this is typically the application's name.</param>
            /// <param name="eventLog">The Windows event log to write into.  Standard logs are Application, Security and System.</param>
            public static void LogSuccessAudit(string eventMessage, int eventID, string eventSource, string eventLog)
            {
                try
                {
                    EventLog.WriteEntry(eventSource, eventMessage, EventLogEntryType.SuccessAudit, eventID);
                }
                catch (Exception)
                {

                }
            }
            #endregion

            #region Static Methods for writing Failure Audit to Security event log
            /// <summary>
            /// Writes a Failure Audit entry into the Windows Security event log.
            /// </summary>
            /// <param name="eventMessage">Descriptive text detailing the nature of the audit.</param>
            /// <param name="eventID">Integer value specifying the event ID.</param>
            public static void LogFailureAudit(string eventMessage, int eventID)
            {
                LogFailureAudit(eventMessage, eventID, Properties.Resources.EventLogTaryn, "Security");
            }

            /// <summary>
            /// Writes a Failure Audit entry into the Windows Security event log.
            /// </summary>
            /// <param name="eventMessage">Descriptive text detailing the nature of the audit.</param>
            /// <param name="eventID">Integer value specifying the event ID.</param>
            /// <param name="eventSource">The event source; this is typically the application's name.</param>
            public static void LogFailureAudit(string eventMessage, int eventID, string eventSource)
            {
                LogFailureAudit(eventMessage, eventID, eventSource, "Security");
            }

            /// <summary>
            /// Writes a Failure Audit entry into the Windows Security event log.
            /// </summary>
            /// <param name="eventMessage">Descriptive text detailing the nature of the audit.</param>
            /// <param name="eventID">Integer value specifying the event ID.</param>
            /// <param name="eventSource">The event source; this is typically the application's name.</param>
            /// <param name="eventLog">The Windows event log to write into.  Standard logs are Application, Security and System.</param>
            public static void LogFailureAudit(string eventMessage, int eventID, string eventSource, string eventLog)
            {
                try
                {
                    EventLog.WriteEntry(eventSource, eventMessage, EventLogEntryType.FailureAudit, eventID);
                }
                catch (Exception)
                {

                }
            }
            #endregion

            #region Static Methods for Registering/Unregistering Event Source
            public static void RegisterEventSource(String sourceName)
            {
                if (IsEventSourceRegistered(sourceName))
                {
                    return;
                }
                EventLog.CreateEventSource(sourceName, Properties.Resources.ApplicationEventLog);
            }

            public static void UnregisterEventSource(String sourceName)
            {
                if (!IsEventSourceRegistered(sourceName))
                {
                    return;
                }
                EventLog.DeleteEventSource(sourceName);
            }

            public static bool IsEventSourceRegistered(String sourceName)
            {
                if (EventLog.SourceExists(sourceName))
                {
                    return true;
                }
                return false;
            }
            #endregion
    }
}
