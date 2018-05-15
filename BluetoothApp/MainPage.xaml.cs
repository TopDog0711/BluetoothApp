using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BluetoothApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            

            BluetoothLEAdvertisementWatcher watcher = new BluetoothLEAdvertisementWatcher();
            watcher.ScanningMode = BluetoothLEScanningMode.Active;
          
            watcher.Received += Watcher_Received; 
            watcher.Start();

        }

        async void UpdateUI(string message)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                OutputBox.Text = message + Environment.NewLine + OutputBox.Text;
              
            });
        }

        private async void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
           
            var manufacturerSections = args.Advertisement.ManufacturerData;
            if (manufacturerSections.Any())
            {
                // Only print the first one of the list
                var manufacturerData = manufacturerSections[0];
                var data = new byte[manufacturerData.Data.Length];
                using (var reader = DataReader.FromBuffer(manufacturerData.Data))
                {
                    reader.ReadBytes(data);
                }

                UpdateUI($"MANDATA = ID: 0x{ manufacturerData.CompanyId.ToString("X")} : {BitConverter.ToString(data)}");
               
            }

            

            var dataSections = args.Advertisement.DataSections;
            if(dataSections.Any())
            {

                foreach(var dataSection in dataSections)
                {
                    var data = new byte[dataSection.Data.Length];
                    using (var reader = DataReader.FromBuffer(dataSection.Data))
                    {
                        reader.ReadBytes(data);
                    }

                    UpdateUI($"DATASECTION = {BitConverter.ToString(data)}");
                  
                }
               
               
            }

            BluetoothLEDevice device = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);
            if(device != null)
            {
                UpdateUI($"DEVICE = Name: { device.Name}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
