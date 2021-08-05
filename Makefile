# varibales
buildpath = build/Output.exe

sourcepath = src/*.cs

# compile
output: $(sourcepath)
	mcs $(sourcepath) -out:$(buildpath)

# clean
clean:
	rm $(buildpath)

# run
run:
	mono $(buildpath)
