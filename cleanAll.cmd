FOR /F "tokens=*" %%G IN ('DIR /B /AD /S bin ^| findstr /v node_modules') DO RMDIR /S /Q "%%G"
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj ^| findstr /v node_modules') DO RMDIR /S /Q "%%G"
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S src ^| findstr /v node_modules') DO DEL "%%G\project.json.bak" /Q
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S src ^| findstr /v node_modules') DO DEL "%%G\project.lock.json" /Q
