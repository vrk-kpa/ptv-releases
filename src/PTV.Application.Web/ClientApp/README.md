# Transifex

Important: **Even though transifex uses same terms (pull, push) it is not git**. Source keys and localizations can be deleted quite easily.

## Preparations

1. Install [CLI](https://developers.transifex.com/docs/cli) and add it to your PATH
2. Login to transifex and create apitoken for yourself (see your user settings)
3. Run `tx pull` in project "ClientApp" directory. This will give instructions on how to get the api key and save it in your home directory.

## Adding new localizations

Notes:

- **Transifex is not Git**. When you push keys/translations you are pushing the whole files. If there is a key missing from that file transifex thinks _Oh you want to remove this key since it is not part of the data you are trying to push_.
- There is a separate configuration for old ui (under `OldApp/.tx`) and new ui (under `ClientBin/.tx`)
- You need to specify the resource otherwise it pulls from dev and prod and last one overwrites the previous pull

Safest way to add new keys:

1. Pull latest keys and translations from transifex overwriting any local changes

   `tx pull -f -s -t fsc-dev.newuijson`

   The `-f` (force) is important as without it cli does not overwrite your local changes.

2. Add only new **keys** to the `af.json` file and push them to transifex

   `tx push -s fsc-dev.newuijson`

3. Go to transifex web ui and add fi and en translations for those keys
4. Pull changes

   `tx pull -f -s -t fsc-dev.newuijson`

   The `-f`(force) is important as without it cli does not overwrite your local changes.

5. Commit your changes
