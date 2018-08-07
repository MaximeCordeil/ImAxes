# ImAxes (1.0 beta)
ImAxes is an immersive data visualisation tool for multidimensional data exploration and analysis in virtual reality.
[link to the original paper](https://www.researchgate.net/publication/318876157_ImAxes_Immersive_Axes_as_Embodied_Affordances_for_Interactive_Multivariate_Data_Visualisation)

<img width="280" alt="iatk_menu" src="https://user-images.githubusercontent.com/11532065/36952172-53002612-2060-11e8-9341-9546a3224092.PNG"> <img width="280" alt="iatk_menu" src="https://user-images.githubusercontent.com/11532065/36952173-533755e2-2060-11e8-99ec-6b2aa150c325.PNG"> <img width="280" alt="scatterplot" src="https://user-images.githubusercontent.com/11532065/36952174-537145a4-2060-11e8-8f2d-bb7e783d13d9.PNG">

From a CSV/TSV spreadsheet, build and explore multivariate/multidimensional data in Virtual Reality.

Here is a video demo of ImAxes:

<a href="https://www.youtube.com/watch?v=hxqJJ934Reg
" target="_blank"><img src="https://img.youtube.com/vi/hxqJJ934Reg/0.jpg" alt="ImAxes"
 width="240" height="180" border="10" /></a>

## Installation instructions
ImAxes comes as a Unity project. You only need to download the latest [Unity 5.6](https://unity3d.com/get-unity/download/archive)

** << Udpate >> : Now ImAxes works with Unity 2018!! check it out: https://github.com/MaximeCordeil/ImAxes/tree/ImAxes2018 big thanks to our student Benjamin Lee for this **

## Hardware and compatibility
ImAxes works on Windows PCs. You will need a solid *gaming* configuration, i.e. an Intel i7 processor and an Nvidia 10xx VR-ready graphics card.

The HTC Vive and Micrsofot-compatible MR devices, and the Oculus Rift CV1 with Oculus touch controllers are currently supported.

## Launching Imaxes
ImAxes allows you to load data with a CSV or a TSV file format. The CSV/TSV dataset file:
  * **must be clean! if you have an empty line, or a line with incorrect data values, ImAxes is very likely to throw parsing exceptions**
  * **should not exceed 65,534 entries**

Your dataset file should look like this:

![snipetdata](https://user-images.githubusercontent.com/11532065/36827716-5ea25eb2-1d69-11e8-8da4-f073c88d3923.PNG)

Each column corresponds to a dimension of the data. Once you have launched ImAxes you will see the corresponding dimensions as virtual axes in space like this:

![imaxesshelf](https://user-images.githubusercontent.com/11532065/37009522-72eea238-213b-11e8-836f-b82cabc8afe9.PNG)


The project contains two template scenes:
  * a ViveScene, to be used with the HTC Vive and Mixed-Reality compatible devices
  * an OculusTouchScene, to be used with the Oculus CV1 and Oculus Touch controllers

Both scenes have a SceneManager Unity gameobject in the hierarchy. Click this object [1] and in the inspector window, you can drag and drop your CSV/TSV file into the *Source Data* field [2]. You can also create a *metadata* file for your dataset and drag and drop it into the *Metadata* field.

![datasource](https://user-images.githubusercontent.com/11532065/36767569-28938eb6-1c8f-11e8-8aa5-984aab9202a7.PNG)

### The metadata file
The metadata file is used to specify the data binning for the histograms corresponding to each data dimension. It is optional and if you do not create one the histograms will have default bins.

To create a metadata file:

1. Right click in the editor (e.g. under Datasets/) and select ImAxes> Data Object Metadata

<img src="https://user-images.githubusercontent.com/11532065/36769170-bf0b159c-1c96-11e8-824a-6d20df94ea6d.jpg" width="40%">

2. Name your metadata file (e.g. datasetname-metadata)
3. Click on the file
4. In the inspector specify the numbers of bins in the *Size* field (it corresponds to the number of dimensions in your dataset)
5. Populate each *Element i* (i: 0 -> nbDimensions-1) bin with the desired bin size for the dimension

### Run ImAxes in the Unity editor
Once you have attached a clean CSV/TSV file to the SceneManager (and optionnally a metadata file), you can run ImAxes in the editor by simply clicking the play button.

### Building and launching ImAxes
You can build and launch the HTCVive or the OculusTouch scene. Make sure you have attached the dataset file in the SceneManager gameobject.

## Using ImAxes

### Create visualisations
Visualisations are created by pulling out axes from the *axes shelf* and by assembling them in space:

  * Hold 2 axes parallel to each other, you get a parallel coordinate.
  * Make 2 axes perpendicular and connect the ends, you get a 2D scatterplot
  * Add a third perpendicular axis, you get a 3D scatterplot
  * Extend visualisation by adding more axes and you will obtain matrices
  * Place visualisations close to each other to connect them

Check the [video](https://www.youtube.com/watch?v=hxqJJ934Reg) to learn the interactions.

### Changing the visualisation styles
Press the *Menu* button on the HTC vive controller (or you Mixed-realiy device) and you will see this menu popup.

*[image menu]*

You can change the color scheme:
  * bind a palette to a categorical dimension
  * bind a gradient color (change min/max colors) to a continuous variable
 
*[image menu]*

You can change the visualisation style:
  * If you have a time series dataset (or a trail set dataset) *and* your data are ordered properly by id in the CSV/TSV source file, you can change the visualisation to show connected dots like this:
*[image menu]*

## Roadmap
This is a *beta version of ImAxes*, it means that it is not bug-free and does not contain all the data visualisation features for now.
We plan to:
  * Add menu in the Oculus Touch scene
  * Make ImAxes work with Unity 2017.x
  * Make ImAxes collaborative (multi user)
  * Have a generic version for a variety of Mixed Reality devices (e.g. Hololens, Meta 2...)
  * Make ImAxes work with (multivariate) graph data
  * Integrate map visualisations
  * Add data selection interaction
  * Better handling of dataset loading in built versions
  * Integrate more control over color mapping: choose color palette / colors in the palette
  * Save the scene and visualisation configuration

## Referencing ImAxes
If you plan to use this software for publication, please cite the paper:

@inproceedings{Cordeil:2017:IIA:3126594.3126613,
 author = {Cordeil, Maxime and Cunningham, Andrew and Dwyer, Tim and Thomas, Bruce H. and Marriott, Kim},
 title = {ImAxes: Immersive Axes As Embodied Affordances for Interactive Multivariate Data Visualisation},
 booktitle = {Proceedings of the 30th Annual ACM Symposium on User Interface Software and Technology},
 series = {UIST '17},
 year = {2017},
 isbn = {978-1-4503-4981-9},
 location = {Qu\&\#233;bec City, QC, Canada},
 pages = {71--83},
 numpages = {13},
 url = {http://doi.acm.org/10.1145/3126594.3126613},
 doi = {10.1145/3126594.3126613},
 acmid = {3126613},
 publisher = {ACM},
 address = {New York, NY, USA},
 keywords = {immersion, immersive analytics, immersive visualization, information visualization, multidimensional data visualization, virtual reality},
}

## Developpers and contributors
ImAxes is mainly designed and developped by [Maxime Cordeil](http://ialab.it.monash.edu/~maxc/) (Monash University) and Andrew Cunningham (University of South Australia).

Contributors: ImAxes an [Immersive Analytics](http://ialab.it.monash.edu/) collaborative research project with 
  * Assoc. Prof [Tim Dwyer](http://ialab.it.monash.edu/~dwyer/), Prof. [Kim Marriott](http://users.monash.edu/~marriott/shadowfax/) (Monash University)
  * Prof. Bruce H. Thomas (University of South Australia).
