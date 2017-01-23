using System;

namespace Gsof.Pdf.Declarations
{
    public interface IPdf : IDisposable
    {
        /// <summary>
        /// 是否加密
        /// </summary>
        bool IsEncrypt { get; }

        /// <summary>
        /// 页数
        /// </summary>
        int Pages { get; }

        /// <summary>
        /// 缩放
        /// </summary>
        float Zoom { get; set; }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="p_source">源</param>
        /// <param name="p_password">密码</param>
        void Open(IPdfSource p_source, string p_password = null);

        /// <summary>
        /// 获取PDF所有页的尺寸
        /// </summary>
        /// <returns></returns>
        Size[] GetPageBounds();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_index">页数</param>
        /// <returns></returns>
        byte[] Extrac(int p_index);
    }
}