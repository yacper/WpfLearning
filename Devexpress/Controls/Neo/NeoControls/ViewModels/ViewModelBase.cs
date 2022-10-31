﻿/********************************************************************
    created:	2022/2/9 17:21:26
    author:		rush
    email:		yacper@gmail.com	
	
    purpose:    基础ViewModel
    modifiers:	
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using DevExpress.Mvvm;

namespace NeoTrader
{
	public abstract class ViewModelBase : DevExpress.Mvvm.ViewModelBase, IDisposable
	{
		public string BindableName
		{
			get { return GetBindableName(DisplayName); }
		}

        public virtual string      DisplayName  { get=>GetProperty(()=>DisplayName); set=>SetProperty(()=> DisplayName, value); }
        public virtual ImageSource Glyph        { get=>GetProperty(()=>Glyph); set=>SetProperty(()=> Glyph, value); }
        public virtual ImageSource StateImg     { get=>GetProperty(()=>StateImg); set=>SetProperty(()=>StateImg, value); } // 状态图标
        public virtual string      BadgeContent { get=>GetProperty(()=>BadgeContent); set=>SetProperty(()=>BadgeContent, value); } // badge

	
		string GetBindableName(string name)
		{
			if (name == null)
				return "";
			return "_" + Regex.Replace(name, @"\W", "");
		}

		#region IDisposable Members

		public void Dispose()
		{
			OnDispose();
		}

		protected virtual void OnDispose()
		{
		}
#if DEBUG
		~ViewModelBase()
		{
			string msg = string.Format("{0} ({1}) ({2}) Finalized", GetType().Name, DisplayName, GetHashCode());
			System.Diagnostics.Debug.WriteLine(msg);
		}
#endif

      
#endregion


    }
}