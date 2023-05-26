using System.Drawing;
using System.Drawing.Imaging;



internal class Program
{
    public struct RGB
{
	private byte _r;
	private byte _g;
	private byte _b;

	public RGB(byte r, byte g, byte b)
	{
		this._r = r;
		this._g = g;
		this._b = b;
	}

	public byte R
	{
		get { return this._r; }
		set { this._r = value; }
	}

	public byte G
	{
		get { return this._g; }
		set { this._g = value; }
	}

	public byte B
	{
		get { return this._b; }
		set { this._b = value; }
	}

	public bool Equals(RGB rgb)
	{
		return (this.R == rgb.R) && (this.G == rgb.G) && (this.B == rgb.B);
	}
}
    public struct HSL
{
	private int _h;
	private float _s;
	private float _l;

	public HSL(int h, float s, float l)
	{
		this._h = h;
		this._s = s;
		this._l = l;
	}

	public int H
	{
		get { return this._h; }
		set { this._h = value; }
	}

	public float S
	{
		get { return this._s; }
		set { this._s = value; }
	}

	public float L
	{
		get { return this._l; }
		set { this._l = value; }
	}

	public bool Equals(HSL hsl)
	{
		return (this.H == hsl.H) && (this.S == hsl.S) && (this.L == hsl.L);
	}
}
    public static HSL RGBToHSL(RGB rgb)
    {
        HSL hsl = new HSL();

        float r = (rgb.R / 255.0f);
        float g = (rgb.G / 255.0f);
        float b = (rgb.B / 255.0f);

        float min = Math.Min(Math.Min(r, g), b);
        float max = Math.Max(Math.Max(r, g), b);
        float delta = max - min;

        hsl.L = (max + min) / 2;

        if (delta == 0)
        {
            hsl.H = 0;
            hsl.S = 0.0f;
        }
        else
        {
            hsl.S = (hsl.L <= 0.5) ? (delta / (max + min)) : (delta / (2 - max - min));

            float hue;

            if (r == max)
            {
                hue = ((g - b) / 6) / delta;
            }
            else if (g == max)
            {
                hue = (1.0f / 3) + ((b - r) / 6) / delta;
            }
            else
            {
                hue = (2.0f / 3) + ((r - g) / 6) / delta;
            }

            if (hue < 0)
                hue += 1;
            if (hue > 1)
                hue -= 1;

            hsl.H = (int)(hue * 360);
        }

        return hsl;
    }

    public static void createLookup(){
        
        Dictionary<string, string> hSGStoresRGB = new Dictionary<string, string>();
        RGB rgb = new RGB();
        HSL hsl = new HSL(); 
        float grayValue;
        Color redGreenBlue;
        
        for (int r = 0; r <= 255; r ++)
        {
            for (int g = 0; g <= 255; g ++)
            {
                for (int b = 0; b <= 255; b ++)
                {
                    rgb.R = (byte)r;
                    rgb.G = (byte)g;
                    rgb.B = (byte)b;
                    hsl = RGBToHSL(rgb);
                    redGreenBlue = Color.FromArgb(r,g,b);
                    grayValue = GrayValueOfColor(redGreenBlue).R;
                    grayValue = grayValue /255 * 100;
                    

                    if(!hSGStoresRGB.ContainsKey($"{hsl.H},{(int)Math.Round(hsl.S*100)},{(int)grayValue}")) 
                    {
                        hSGStoresRGB.Add($"{hsl.H},{(int)Math.Round(hsl.S*100)},{(int)grayValue}",$"{r},{g},{b}");
                    }

                }
                
            }
            
        }

    
    }

    private static void Main(string[] args)
    {
        Bitmap gray  = new Bitmap("./gray.png");
        Bitmap color = new Bitmap("./color.png");
        Bitmap final = new Bitmap(gray.Width, gray.Height);

        for(int y = 0; y < gray.Height; y++)
        {
            for(int x = 0; x < gray.Width; x++)
            {
                Color grayPixelColor = gray.GetPixel(x,y);
                Color colorPixelColor = color.GetPixel(x,y);
                Color newColor = NewColor(grayPixelColor, colorPixelColor);
                final.SetPixel(x, y, newColor);
            }
        }  

        final.Save("adjusted.png", ImageFormat.Png);  
    }

    private static Color NewColor(Color gray, Color color){
        if(gray == GrayValueOfColor(color)) return color;
        bool isBrighter = false;
        if(gray.R < GrayValueOfColor(color).R) isBrighter = true;

        while(isBrighter)
        {
            if(gray.R >= GrayValueOfColor(color).R) return color;
            color = DarkenColor(color);
        }
        while(!isBrighter)
        {
            if(gray.R <= GrayValueOfColor(color).R) return color;
            color = BrightenColor(color);
        }
        return color;
    }
    private static Color GrayValueOfColor(Color color){
        int grayValue = (int)((color.R * 0.3) + (color.G * 0.59) + (color.B * 0.11));
        return Color.FromArgb(grayValue,grayValue,grayValue);
    }
    private static Color BrightenColor(Color color){
        Color brighterColor = Color.FromArgb(
            color.R < 255 ? color.R + 1 : color.R,
            color.G < 255 ? color.G + 1 : color.G,
            color.B < 255 ? color.B + 1 : color.B
        );
        return brighterColor;
    }
    private static Color DarkenColor(Color color){
        Color darkerColor = Color.FromArgb(
            color.R > 0 ? color.R - 1 : color.R,
            color.G > 0 ? color.G - 1 : color.G,
            color.B > 0 ? color.B - 1 : color.B
        );
        return darkerColor;
    }
    
}