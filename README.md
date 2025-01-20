# Timspect
This project allows unpacking and repacking various playstation TIM texture types.  
It also has a viewer for viewing different TIM textures.  

The unpacker allows for editing various properties of the texture in an XML before repacking; this ensures the correct data is repacked.  
The unpacker also automatically handles reducing color count when a texture type requires it for repacking.  

Currently only TIM2 is supported for unpack and repack.  
Currently the unpacker only handles PNG for repacking, this is so that index and palette data is perfectly preserved on unpack and repack.   

The project is currently focused on FromSoftware TIM2 files, but also supports normal TIM2 files.

# Building
This project requires the following libraries to be cloned alongside it.  
Place them in the same top-level folder as this project.  
```
git clone https://github.com/WarpZephyr/BinaryMemory.git  
```