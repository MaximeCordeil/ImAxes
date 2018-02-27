# ImAxes
ImAxes is an immersive data visualisation tool for multidimensional data exploration and analysis in virtual reality.
[link to the original paper](https://www.researchgate.net/publication/318876157_ImAxes_Immersive_Axes_as_Embodied_Affordances_for_Interactive_Multivariate_Data_Visualisation)

Have a look at what ImAxes does:

<a href="https://www.youtube.com/watch?v=hxqJJ934Reg
" target="_blank"><img src="https://img.youtube.com/vi/hxqJJ934Reg/0.jpg" alt="ImAxes"
 width="240" height="180" border="10" /></a>

## Installation Instructions
ImAxes comes as a Unity project. You only need to download the latest [Unity 5.6](https://unity3d.com/get-unity/download/archive)


## Hardware and compatibility
ImAxes works on Windows PCs. You will need a solid *gaming* confirmation, i.e. an Intel i7 processor and an Nvidia 10xx VR-ready graphics card.

The software currently supports 

## Starting Imaxes
ImAxes allows you to load data with a CSV or a TSV file format. 
**Your CSV/TSV data file must be clean! if you have an empty line, or a line with incorrect data values, ImAxes is very likely to throw parsing exceptions**



### In the editor
The ViveScene and the OculusTouchScene have 

### Building and launching ImAxes
You can

## Roadmap
This is a *beta version of ImAxes*, it means that it is not bug-free and does not contain all the data visualisation features for now.
We plan to:
1. Make ImAxes work with Unity 2017.x
2. Have a generic version for a variety of Mixed Reality devices (e.g. Hololens, Meta 2...)
3. Integrate map visualisations
4. Add data selection interaction

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
