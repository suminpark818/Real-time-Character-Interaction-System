// CubismParameterExtension.cs
using Live2D.Cubism.Core;

namespace Live2D.Cubism.Framework
{
    public static class CubismParameterExtension
    {
        // 구버전 호환용 Dummy 속성
        public static bool DidChange(this CubismParameter parameter)
        {
            return false;
        }

        public static void DidChange(this CubismParameter parameter, bool value)
        {
            // Live2D SDK 4.2 미만 버전은 내부적으로 자동 반영하므로 비워둔다.
        }
    }
}
