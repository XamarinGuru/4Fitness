using System;

namespace goheja
{
	public class myApp:ApplicationException
	{
		

		private String myGlobalState;

		public String getGlobalState(){
			return myGlobalState;
		}
		public void setGlobalState(String s){
			myGlobalState = s;
		}

	}
}

