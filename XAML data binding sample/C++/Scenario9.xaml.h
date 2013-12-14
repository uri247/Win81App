//
// Scenario9.xaml.h
// Declaration of the Scenario9 class
//

#pragma once

#include "Scenario9.g.h"

namespace SDKSample
{
	namespace DataBinding
	{
		/// <summary>
		/// An empty page that can be used on its own or navigated to within a Frame.
		/// </summary>
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class Scenario9 sealed
		{
		public:
			Scenario9();

		private:
			property Employee^ _employee;
			//MainPage^ rootPage;
			void ScenarioReset(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void EmployeeChanged(Object^ sender, PropertyChangedEventArgs^ e);
			void UpdateDataBtnClick(Object^ sender, PropertyChangedEventArgs^ e);
		};
	}
}


