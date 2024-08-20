
@echo off
echo Updating ZIP files...
del *.zip
for /d %%1 in (*) do 7z a mwnet%%1.zip ./%%1/rockaway/* -mx0 -xr!bin -xr!obj -xr!_NCrunch_Rockaway -xr!.idea -xr!.vs -xr!*.ncrunch*
echo All done! Yay!