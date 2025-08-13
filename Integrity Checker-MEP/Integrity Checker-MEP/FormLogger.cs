using System;

namespace Integrity_Checker_MEP
{
    public class FormLogger : ILogger
    {
        private readonly From_Log _logForm;

        public FormLogger(From_Log logForm)
        {
            _logForm = logForm;
        }

        public void Log(string message)
        {
            if (_logForm != null && !_logForm.IsDisposed)
            {
                if (_logForm.InvokeRequired)
                {
                    _logForm.Invoke(new Action(() => _logForm.UpdateLog(message)));
                }
                else
                {
                    _logForm.UpdateLog(message);
                }
            }
        }
    }
}
