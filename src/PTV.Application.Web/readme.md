# Transifex

Important: **Even though transifex uses same terms (pull, push) it is not git**. Source keys and localizations can be deleted quite easily.

## Preparations

1. Install [CLI](https://developers.transifex.com/docs/cli) and add it to your PATH
2. Login to transifex and create apitoken for yourself (see your user settings)
3. Create temporary folder and execute following commands there. This will create `.transifexrc` file to your home folder and it will contain the apikey

    `tx init`

    `tx add`

  After inputting the apikey you can `Ctrl+C`. You can now delete the temporary folder.

## Adding new localizations

**Think of adding localization keys as a single thing that you perform from start to finish.** 

E.g. as follows:

1. Pull latest keys and translations from transifex
2. Add your own keys and commit in version control/stash
3. Pull latest keys again and solve possible conflicts (e.g. if you stashed your changed and want to replay them)
4. Push your changes to transifex

This is because **transifex is not Git**. When you push keys/translations you are pushing the whole files. If there is a key missing from that file transifex things _Oh you want to remove this key since it is not part of the data you are trying to push_. **If there is one hour between you pulling latest from transifex and pushing your own changes, chances are that keys/localizations are removed from transifex.**

Go to `wwwroot` folder and pull all the existing localizations and source keys from transifex. The `--force` is important. Without it CLI does not overwrite your local changes and when you push changes back to transifex keys can be lost.

    tx pull --translations --source --force
    or
    tx pull -t -s -f

Add new keys into `noTranslation.json` and your localizations into `Fi.json` and `En.json`. After that push the changes to transifex:

    tx push --translations --source
    or
    tx push -t -s

