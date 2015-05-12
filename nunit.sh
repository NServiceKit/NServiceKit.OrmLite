runTest(){
	mono lib/tests/nunit-console/nunit-console.exe -noxml -nodots -labels -stoponerror $@
	if [ $? -ne 0 ]
	then
		exit 1
	fi
}

runTest $1 -exclude=Integration,Performance
exit $?
