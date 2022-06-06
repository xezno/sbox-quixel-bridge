# Quixel Bridge plugin for s&box

Quickly and easily import Quixel assets using Quixel Bridge.

https://user-images.githubusercontent.com/12881812/163437857-7d42e4f6-4533-4c68-a661-aec74a4c976a.mp4

## Disclaimer

Please be aware that the 'free' Quixel tier is for use with Unreal Engine only.
This tool should only be used with either the 'Personal', 'Indie', or 'Studio' Quixel tiers.
More information about these is located [here](https://quixel.com/pricing).

I am not affiliated with Quixel, Epic Games, or Facepunch.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## Supported Assets

| Asset type | Supported? |
|-|-|
| 3D Assets | ✅ |
| 3D Asset Packs | ❌ |
| Surfaces | ✅ |
| Decals | ✅ |
| Atlases | ❌ |
| Imperfections | ✅ |
| Displacements | ✅ |
| Brushes | ✅ |


## Setup

You'll only need to do this stuff once.

### s&box setup

1. Download the plugin from [GitHub](https://github.com/xezno/sbox-quixel-bridge/releases/latest).
2. Extract it somewhere permanent, e.g. next to all your other s&box addons.
3. Add it through the s&box addon manager window ("Add" -> "From folder..").
4. Click "Quixel" -> "Settings" in the s&box editor and select the addon you want to export to.
5. Click "Save and Close" to apply the settings you've chosen.

### Bridge Setup

1. Download Quixel Bridge from the [Quixel website](https://quixel.com/bridge).
2. Open Quixel Bridge.
3. Open "Edit" -> "Export Settings":
   - Change "Export Target" to "Custom Socket Export"
   - Open the "Textures" tab  and select "PNG" as the texture format

## Usage

After you've set everything up, using it is straightforward.

1. Make sure the plugin is enabled in s&box - go into the "Quixel" menu in the editor, and hit "Start Bridge Plugin".
2. Go to Quixel Bridge.
3. Select an asset, click the green "Download" button.
4. Once downloaded, the click the blue "Export" button.

You can find your exported assets exported in the "megascans" directory in the root folder for the addon you picked.
