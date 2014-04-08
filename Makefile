main:
	mcs -r:libs/sfmlnet-audio-2.dll -r:libs/sfmlnet-graphics-2.dll -r:libs/sfmlnet-window-2.dll -platform:x86 -main:JohnnySpaceGame.EntryPoint -out:build/spehss.exe src/*.cs
	copy libs\*.dll build
	copy media\* build

clean:
	del build\*