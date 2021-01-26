rm -f *.trx
for i in $(find /vsoagent/work/1/s/test -mindepth 1 -maxdepth 1 -type d); do 
    rm -f $i/TestResults/*.trx
    PROJ=${i##*/}
    dotnet test $i/$PROJ.csproj --logger:trx 2>$i/$PROJ.Error.log
    cp $i/TestResults/*.trx .
done
