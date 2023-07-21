using System.ComponentModel;

namespace Cleanception.Domain.Common;

public enum FileType
{
    [Description(".jpg,.png,.jpeg")]
    Image,
    [Description(".pdf")]
    PdfFile
}