using System.Collections.Generic;

namespace GalacticScale.Generators
{
    public class DemoGenerator : iConfigurableGenerator
    {
        public string Name => "Demo";
        public string Author => "innominata";
        public string Description => "Sample Generator that doesn't do a lot";
        public string Version => "0.0";
        public string GUID => "space.customizing.generators.demo";
        public GSGeneratorConfig Config => config; // Return our own generator config when asked, instead of using default config
        public GSOptions Options => options; // Likewise for options
        public bool DisableStarCountSlider => false;
        public void Init()
        {
            GS2.Log("Initializing Demo Generator"); // Use Galactic Scales Log system for debugging purposes.
            config.DisableSeedInput = true;
            config.DisableStarCountSlider = false;
            config.MaxStarCount = 1024; //1024 is game limit, and already ridiculous. Setting this higher will cause the game to crash.
            config.MinStarCount = 1;
            List<string> densityList = new List<string>() { "Densest", "Denser", "Default", "Sparse", "Sparsest" };
            densityCombobox = options.Add(GSUI.Combobox("Density", densityList, DensitySelectCallback, InitializeDensitySelect));
        }
        public void Import(GSGenPreferences prefs) //This is called on game start, with the same object that was exported last time
        {
            preferences = prefs;
            SetDensity(prefs.GetInt("Density", 0)); // GSGenPreferences has some helper methods such as a guaranteed integer return, with default.
        }
        public GSGenPreferences Export() // Send our custom preferences object to be saved to disk
        {
            return preferences;
        }
        public void Generate(int starCount) // Redirect to our private generation method
        {
            generate(starCount);
        }
        //////////////////////////////////////////////////////////////////////
        ///// All code below here is generator specific
        //////////////////////////////////////////////////////////////////////
        private float minStepLength = 2.3f;
        private float maxStepLength = 3.5f;
        private float minDistance = 2f;
        private GSUI densityCombobox;
        public GSOptions options = new GSOptions();
        private GSGenPreferences preferences = new GSGenPreferences();
        private GSGeneratorConfig config = new GSGeneratorConfig();

        ////////////////////////////
        /// Handle Settings Elements
        ////////////////////////////
        public void InitializeDensitySelect() // UI Element Postfix methods are called once UI elements have been created
        {
            densityCombobox.Set(preferences.GetInt("Density",0)); //Set the index of the combobox to the integer saved in our preferences
        }
        public void DensitySelectCallback(object o) => SetDensity((int)o); // Callback methods are called when the user interacts with the UI element

        //////////////////////////////
        /// Finally, lets do something
        //////////////////////////////

        public void SetDensity(int index)
        {
            preferences.Set("Density",index);
            int i = index;
            switch (i)
            {
                case 0: minStepLength = 1.2f; maxStepLength = 1.5f; minDistance = 1.2f; break;
                case 1: minStepLength = 1.6f; maxStepLength = 2.5f; minDistance = 1.7f; break;
                case 3: minStepLength = 2.2f; maxStepLength = 5.0f; minDistance = 2.2f; break;
                case 4: minStepLength = 3.0f; maxStepLength = 7.0f; minDistance = 3.0f; break;
                default: minStepLength = 2f; maxStepLength = 3.5f; minDistance = 2.3f; break;
            }
        }

        public void generate(int starCount)
        {
            SetDensity(preferences.GetInt("Density",2));

            //Create a list containing a single planet, that has default values.
            GSPlanets planets = new GSPlanets()
            {
                new GSPlanet("Urf", "Mediterranean", 200, 1, 0, 0, 1000, 0, 0, 1000, 0, 1)
            };
            planets[0].Radius = 5; //Lets make it tiny! (Vanilla size is 200).

            //Create one O-type main sequence star, containing the above planet. Set the seed to one. This is the minimum requirement for a star. There are more options available.
            GSSettings.Stars.Add(new GSStar(1, "BeetleJuice", ESpectrType.O, EStarType.MainSeqStar, planets)); 
            //Create a whole bunch of identical empty F-type Giant stars
            for (var i = 1; i < starCount; i++)
            {
                GSSettings.Stars.Add(new GSStar(1, "Star" + i.ToString(), ESpectrType.F, EStarType.GiantStar, new GSPlanets()));
            }

            //Change some of the galaxy parameters. This is not necessary, but it lets this test generator actually do something meaningful to affect generation.
            GSSettings.GalaxyParams.minDistance   = minDistance;
            GSSettings.GalaxyParams.minStepLength = minStepLength;
            GSSettings.GalaxyParams.maxStepLength = maxStepLength;
        }
    }
}